let socket = new WebSocket("wss://localhost:5001/ws");

socket.onopen = function(e) {
  console.log("[open] Connection established");
};

socket.onmessage = function(event) {
  console.log(`[message] Data received from server: ${event.data}`);
};

function start()
{
    let xhttp = new XMLHttpRequest();
    xhttp.open("GET","https://localhost:5001/api/FakeDevice/Start",false)
    xhttp.send();
    console.log("Start sent")
}

function stop()
{
    let xhttp = new XMLHttpRequest();
    xhttp.open("GET","https://localhost:5001/api/FakeDevice/Stop",false)
    xhttp.send();
    console.log("Stop sent")
}
function init()
{
    let xhttp = new XMLHttpRequest();
    xhttp.open("GET","https://localhost:5001/api",false)
    xhttp.send();
    console.log("Init sent")
}