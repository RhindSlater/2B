function loading() {
    $("#loader-wrapper").css("display", "block");
}

function checkNotifications() {
    var i;
    $.ajax({
        url: '/Users/CheckNotifications',
        success: function(data) {
             for(i = 0; i < data.length; i++){
                if(i == 10){
                    
                    break;
                }
                var a = document.createElement('a');
                a.setAttribute("class","dropdown-item");
                var item = data[i].value;
                $("#notification-div").appendChild(a);

             }
             var test = data;
             console.log(data);
             alert(data);
        }
    });
}