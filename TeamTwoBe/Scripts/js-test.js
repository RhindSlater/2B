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

function RequestCard(id){
    $.ajax({
        url: '/Users/requestTrade/' + id,
        success: function(data) {
            alert(data);
        }
    });
}

var intervalID = window.setInterval(checkNotifications, 5000);
var intervalID2 = window.setInterval(Checkshoppingcart, 5000);

$(document).ready(function(){
    checkNotifications();
    Checkshoppingcart();
    displayHomeCards();
});

function Checkshoppingcart(){
    $.ajax({
        url: '/Users/CheckShoppingCount',
        success: function(data){
            $("#shoppingcount").val(data);
        },
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
            var count = 0;
             for(i = 0; i < data.length; i++){
                //only show 10 notifications
                if(data[i].Seen == false){   
                    count ++;
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
             var i = $('.badge');
             if(count > 0){
                i.text(count);
             }
             else{
                 
             }
        }
    });
}

function removeSeen(id){
    var count;
    count = $('.badge');
    count = count[0].textContent;
    var li = $("#notification-here > #anchor-notification");
    li[id].setAttribute("class","dropdown-item");
    $.ajax({
        url: '/Users/ChangeNotification/' + id,
        success: function(data) {
            if(data == "true"){
                console.log("Notification changed successfully");
                var i = $('.badge');
                i.text(count - 1);
            }
            else{
                console.log("Failed");
            }
        }
    });
}

function SearchCard(){
    var name = $('#dropbox').val();
    var i;
    $.ajax({
        method: 'POST',
        url: '/sales/apiPrice/',
        data: { 
            'dropboxvalue': name
        },
        success: function(data) {
            console.log(data.length);
            for(i = 0; i < data.length; i++){
                var op = document.createElement('option');
                op.setAttribute("value", name + " " + data[i].print_tag + " " + data[i].rarity );
                document.getElementById("CardList").appendChild(op);
                document.getElementById("CardList2").appendChild(op);
                console.log(name + " " + data[i].print_tag + " " + data[i].rarity);
            }
            if(data.length == 0){
                console.log("No data returned");
            }
        }
    });
}

function sendbid(){
    var bid;
    bid = $('#bid').val();
    if(bid == ""){
        alert("Insert a bid")
    }
    var a = Number(bid);
    if(a != NaN){
        $.ajax({
            method: 'POST',
            url: '/Auction/placebid/',
            data: { 
                'id': bid
            },
            success: function(data) {
               alert(data);
            },
            fail: function(data){
                alert("Only numbers valid");
            },
        });
    }
    else{
        alert("Insert a number");
    }
}

var auctionTimer = 60;
function AuctionTimer(){
    auctionTimer -= 1;
    var progress = $('#pgbar');
    var w = progress[0].attributes[4].value;
    $('#pgbar').attr('aria-valuetext', w - 6.35);
    $('#pgbar').css('width', w - 6.35 );
    if(auctionTimer == 0){
        $.ajax({
            url: '/Auction/AuctionEnd/',
            method: 'POST',
            success: function(data){
                alert(data);
                if(data == "Auction ended"){
                    StartNewAuction();
                }
            }
        })
    }
}

function StartNewAuction(){
    auctionTimer = 60;
    $('#pgbar').attr('aria-valuetext', 381);
    $('#pgbar').css('width', 381 );
    $.ajax({
        url: '/Auction/AuctionNew',
        method: 'POST',
        success: function(data){
            $('#Auction-Image').attr("src",data.Card.image_url);
            $('#Auction-Image2').attr("src",data.Card.image_url);
        }
    })
}

var rated = 0;
function CRate(r) {
    $("#Rating").val(r);
    for (var i = 1; i <= r; i++) {
        $("#Rate" + i).attr('class', 'starGlow');
        rated = r;
        $("#rating-value").val(r);
    }
    // unselect remaining
    for (var i = r + 1; i <= 5; i++) {
        $("#Rate" + i).attr('class', 'starFade');
    }
}
function SubmitComment() {
    if ($("#Rating").val() == "0") {
        alert("Please rate this service provider.");
        return false;
    }
    else {
        return true;
    }
}

function CRateOver(r) {
    for (var i = 1; i <= r; i++) {
        $("#Rate" + i).attr('class', 'starGlow');
    }
}

function CRateOut(r) {

    for (var i = 1; i <= r; i++) {
        if(i > rated){
            $("#Rate" + i).attr('class', 'starFade');
        }
    }
}

function CRateSelected() {
    var setRating = $("#Rating").val();
    for (var i = 1; i <= setRating; i++) {
        $("#Rate" + i).attr('class', 'starGlow');
    }
}

function displayCorrectCards() {
    displayProfileCards(1);
    displayProfileCards(2);
    displayProfileCards(3);
    displayProfileCards(4);
}
function displayHomeCards() {
    displayProfileCards(1);
    displayProfileCards(3);
    displayProfileCards(2);
}

function displayProfileCards(id){
    var width = window.innerWidth;
    var widthcounter;
    var li = $(".row" + id + " > #CardGrid");
    if (width < 1178) {
        for (widthcounter = 0; widthcounter < 10; widthcounter++) {
            if (widthcounter > 5) {
                li[widthcounter].style.display = 'none';
            }
            else {
                li[widthcounter].style.display = 'block';
            }
        }
    }
    if (width >= 1178) {
        // show 8
        for (widthcounter = 0; widthcounter < 10; widthcounter++) {
            if (widthcounter > 7) {
                li[widthcounter].style.display = 'none';
            }
            else {
                li[widthcounter].style.display = 'block';
            }
        }
    }
    if (width > 1477) {
        // show 10
        for (widthcounter = 0; widthcounter < 10; widthcounter++) {
            if (widthcounter > 9) {
                li[widthcounter].style.display = 'none';
            }
            else {
                li[widthcounter].style.display = 'block';
            }
        }
    }
}
