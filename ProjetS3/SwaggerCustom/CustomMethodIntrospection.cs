using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using ProjetS3.PeripheralCreation;
using IDeviceLib;

namespace ProjetS3.SwaggerCustom
{
    class CustomMethodIntrospection : IDocumentFilter
    {
        private const string UID = "aizjeiuazhneuiabzudbazlekzbzubnadkuz"; 
        private const string API_START_PATH = "/api/";
        private const string API_END_PATH= "/";

        private const string API_TAG_NAME = "BrowserRequests";
        private const string SUCCESS_DESCRIPTION = "Success";
        private const string FAILURE_DESCRIPTION = "Failure";

        private const string SUCCESS_HTTP_CODE = "200";
        private const string FAILURE_HTTP_CODE = "400";

        void IDocumentFilter.Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {

            List<string> allRoutes = generateAllRoutes();

            int counter = 0;

            //Tag Handling
            OpenApiTag tag = new OpenApiTag { Name = API_TAG_NAME};
            List<OpenApiTag> tagList = new List<OpenApiTag>();
            tagList.Add(tag);

            //Response handling

            //Creating the responses indidually
            OpenApiResponse positiveAnswer = new OpenApiResponse { Description = SUCCESS_DESCRIPTION};
            OpenApiResponse negativeAnswer = new OpenApiResponse { Description = FAILURE_DESCRIPTION};

            //Generating the list of answers
            OpenApiResponses allPossibleAnswers = new OpenApiResponses();
            allPossibleAnswers.Add(SUCCESS_HTTP_CODE,positiveAnswer);
            allPossibleAnswers.Add(FAILURE_HTTP_CODE, negativeAnswer);


            foreach (string route in allRoutes)
            {
                Dictionary<OperationType,OpenApiOperation> operationDictionnary = new Dictionary<OperationType,OpenApiOperation>();
                
                OpenApiOperation ope = new OpenApiOperation {OperationId = UID+counter, Tags = tagList, Responses = allPossibleAnswers};
                ++counter;

                operationDictionnary.Add(OperationType.Get, ope);
                swaggerDoc.Paths.Add(route, new OpenApiPathItem{Operations = operationDictionnary});
            }
        }

        //Goal : generates all paths possible since swwagger can't do it dynamically
        //Getting all the instances of the peripherals -> then finding all the methods
        List<string> generateAllRoutes()
        {
            List<string> routesItemsList = new List<string>();
            IList<string> peripheralNames = PeripheralFactory.GetAllInstanceNames();

            foreach(string peripheralName in peripheralNames)
            {

                IDevice currentPeripheralInstance = PeripheralFactory.GetInstance(peripheralName);

                List<MethodInfo> methodList = PeripheralFactory.FindMethods(currentPeripheralInstance.GetType());

                foreach(MethodInfo currentMethod in methodList)
                {
                    if (currentMethod.Name.StartsWith("get_") || currentMethod.Name.StartsWith("set_")) continue;
                    string current = API_START_PATH + peripheralName + API_END_PATH;
                    current += currentMethod.Name;
                    routesItemsList.Add(current);
                    /** Testing purpose -> will work when facto fixed (maybe)
                    ParameterInfo[] currentMethodParameters = currentMethod.GetParameters();
                    foreach(ParameterInfo pi in currentMethodParameters)
                    {
                        Console.WriteLine("" + currentMethod.Name + " Param : " + pi);
                    }
                    */
                }
            }
            return routesItemsList;
        }
    }
}
