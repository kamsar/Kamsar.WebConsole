var CS = CS || {};

CS.SetProgress = function (percent) {
    document.getElementById('bar').style.width = percent + '%';
    document.getElementById('percentage').innerHTML = percent + '%';
}

CS.SetStatus = function (status) {
    document.getElementById('status').innerHTML = status;
}

CS.WriteConsole = function (text) {
    var console = document.getElementById('console');
    console.innerHTML += text;
        
    var scrollHeight = Math.max(console.scrollHeight, console.clientHeight);
    console.scrollTop = scrollHeight - console.clientHeight;
}