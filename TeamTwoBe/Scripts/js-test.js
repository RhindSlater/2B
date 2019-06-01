function loading() {
    $("#loader-wrapper").css("display", "block");
}

function FollowUser(id){
    $.ajax({
        url: '/Users/Follow/' + id,
        success: function(data) {
            alert(data);
        }
    });
}

function BoughtCard(id){
    $.ajax({
        url: '/Profile/purchaseCard/',
        success: function(data) {
            alert(data);
        }
    });
}

function checkNotifications() {
    var i;
    $.ajax({
        url: '/Users/CheckNotifications',
        success: function(data) {
            var myNode = document.getElementById("notification-here");
            while(myNode.firstChild){
                //removes all notifications from element
                myNode.removeChild(myNode.firstChild);
            }

             for(i = 0; i < data.length; i++){
                //only show 10 notifications
                if(i == 10){   
                    //send user to view all notifications                
                    break;
                }

                // declare all the elements needed
                var div = document.createElement('div');
                var img = document.createElement('img');
                var para = document.createElement('p');
                var a = document.createElement('a');
                var br = document.createElement('br');
                
                //build image
                //give image id so we can edit the css
                img.setAttribute("id","notify-pic");
                //set the image url
                img.setAttribute("src","/Properties/" + data[i].NotifyUser.DisplayPicture);


                // build notification message
                //give the notification a title
                para.append(data[i].Title);
                //new line
                para.append(br);
                //attach the notification message
                para.append(data[i].Message);
                //give p id so we can edit the css
                para.setAttribute("id","para-notification");


                // build div
                //give div id so we can edit the css
                div.setAttribute("id","notification");
                //attach the image to div
                div.append(img);
                //attach the title and message
                div.append(para);


                //attach div to anchor
                a.append(div);
                //make anchor a dropdown item
                a.setAttribute("class","dropdown-item");
                //give a id so we can edit the css
                a.setAttribute("id","anchor-notification")
                //add everything into our document
                document.getElementById("notification-here").appendChild(a);

             }
             var test = data;
        }
    });
}

var nav = $("#notification-here");
nav.find("li").each(function() {
    if ($(this).find("ul").length > 0) {

        $("<#notification-div").appendTo($(this).children(":first"));

        //show subnav on hover
        $(this).click(function() {
            $(this).find("ul").stop(true, true).slideToggle();
        });
    }
});
