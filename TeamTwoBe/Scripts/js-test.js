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
                //check if notification has been seen
                if(data[i].Seen == false){
                    a.setAttribute("class","seen dropdown-item");
                }
                else{ //make anchor a dropdown item
                    a.setAttribute("class","dropdown-item");
                }
                //test
                a.setAttribute("onclick","removeSeen(" + i + ")");
                //give a id so we can edit the css
                a.setAttribute("id","anchor-notification");
                //add everything into our document
                document.getElementById("notification-here").appendChild(a);

             }
        }
    });
}

function removeSeen(id){
    var li = $("#notification-here > #anchor-notification");
    li[id].setAttribute("class","dropdown-item");
    $.ajax({
        url: '/Users/ChangeNotification/' + id,
        success: function(data) {
            if(data == "true"){
                console.log("Notification changed successfully");
            }
            else{
                console.log("Failed");
            }
        }
    });
}