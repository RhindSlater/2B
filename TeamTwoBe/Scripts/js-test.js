function loading() {
    $("#loader-wrapper").css("display", "block");
}

function checkNotifications() {
    var i;
    $.ajax({
        url: '/Users/CheckNotifications',
        success: function(data) {
            var myNode = document.getElementById("notification-div");
            // while(myNode.firstChild){
            //     myNode.removeChild(myNode.firstChild);
            // }
             for(i = 0; i < data.length; i++){
                if(i == 10){                   
                    break;
                }
                console.log(data[i]);
                var a = document.createElement('a');
                a.append(data[i].Title);
                a.setAttribute("class","dropdown-item");
                document.getElementById("notification-div").appendChild(a);
                var item = data[i].value;

             }
             var test = data;
             console.log(data);
        }
    });
}

var nav = $("#notification-div");
nav.find("li").each(function() {
    if ($(this).find("ul").length > 0) {

        $("<#notification-div").appendTo($(this).children(":first"));

        //show subnav on hover
        $(this).click(function() {
            $(this).find("ul").stop(true, true).slideToggle();
        });
    }
});
