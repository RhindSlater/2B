!function(window){
    
    function loadImage($el, $fn) {
        jQuery($el).attr('src', jQuery($el).attr('data-src'));
        $fn ? $fn() : null;
    }
    
    function elementInViewport($el) {
        var $rect = $el.getBoundingClientRect();
        return ($rect.top >= 0 && $rect.left >= 0 && $rect.top <= (window.innerHeight || document.documentElement.clientHeight)
        );
    }
    
    jQuery(document).ready(function() {
        
        var images = new Array()
        , query = jQuery('img.lozad')
        , processScroll = function() {
            jQuery.each(images, function(i, img) {
                if (elementInViewport(img)) {
                    loadImage(img, function () {
                        images.splice(i, 1);
                    });
                }
            });
        };
        
        query.each(function() {
           images.push(this);
        });
        
        processScroll();
        jQuery(window).bind('scroll', processScroll);
        
    });
    
}(this);