var customAnimations = (function () {



    return {
        startAnimations: function () {
            var elem = document.getElementById('movementCanvas');
            const maxWidth = window.innerWidth;
            const maxHeight = window.innerHeight * .4;
            const rectWidth = 100;
            const rectHeight = 100;
            const startPosition = rectWidth + maxWidth;
            const bgElemVelocity = .5;

            var params = { width: maxWidth, height: maxHeight };
            var two = new Two(params).appendTo(elem);



            var theRoad = two.makeRectangle(0, maxHeight, maxWidth*2, 100);
            theRoad.fill = 'rgb(169,169,169)';
            theRoad.opacity = 1;
            theRoad.noStroke();

            var kev = two.makeRectangle(maxWidth/2, (maxHeight / 1.6), rectWidth, rectHeight);
            kev.fill = 'rgb(0,0, 255)';
            kev.opacity = 1;
            kev.noStroke();
            var landmark1 = two.makeRectangle(startPosition, (maxHeight / 2), rectWidth, rectHeight);
            landmark1.fill = 'rgb(0, 200, 255)';
            landmark1.opacity = 1;
            landmark1.noStroke();

            
            var group = two.makeGroup(landmark1);

            // And have translation, rotation, scale like all shapes.
            //group.translation.set(two.width / 2, two.height / 2);
            two.bind('update', function (frameCount) {
                if (group.translation.x <= -startPosition) {
                    group.translation.x = 0;
               }
      

                group.translation.x -= 10 * bgElemVelocity
            }).play();
        }
    }

})();