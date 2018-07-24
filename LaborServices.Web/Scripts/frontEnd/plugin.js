$(function () {
    'use strict';


    // fixed menu

    //$(window).scroll(function () {
    //    var upperBar = $('.upper-bar').height();
    //    if ($(window).scrollTop() >= upperBar) {
    //        $('nav').addClass('fixed-top');
    //    }
    //    else {
    //        $('nav').removeClass('fixed-top');
    //    }
    //});


    //// dropdown hover

    //$(".dropdown").hover(
    //    function () {
    //        $(this).addClass("open");
    //        $(".dropdown-menu").animate({}, 300);
    //    }, function () {
    //        $(this).removeClass("open");
    //        $(".dropdown-menu").animate({}, 10);
    //    }
    //);


    // slider img height

    function setsize() {
        if ($(window).width() < 767) {
            $(".slider img").height('auto');
        } else {
            var win_hei = $(window).height(),
                heed = $("header").height();
            $(".slider img").height(win_hei - (heed));
        }
    }

    setsize();
    $(window).resize(function () {
        setsize();
    });


    // testimonial

    //$('.testimonialCarousel').owlCarousel({
    //    rtl: true,
    //    margin: 15,
    //    smartSpeed: 1500,
    //    nav: false,
    //    navText: ['<i class="fa fa-angle-left"></i>', '<i class="fa fa-angle-right"></i>'],
    //    dots: false,
    //    loop: true,
    //    autoplay: true,
    //    mouseDrag: true,
    //    touchDrag: true,
    //    responsive: {
    //        0: {
    //            items: 1
    //        },
    //        600: {
    //            items: 1
    //        },
    //        1000: {
    //            items: 1
    //        }
    //    }
    //});
    // stop on hover

    $('.owl-carousel .item').on('mouseover', function (event) {
        event.preventDefault();
        $(".gallery_carousel").trigger('stop.owl.autoplay');
    });
    $('.owl-carousel .item').on('mouseleave', function (event) {
        event.preventDefault();
        $(".gallery_carousel").trigger('play.owl.autoplay');
    });

    // Custom Navigation Events

    $(".bigNextBtn").click(function () {
        $('.testimonialCarousel').trigger('next.owl.carousel');
    });
    $(".bigPrevBtn").click(function () {
        $('.testimonialCarousel').trigger('prev.owl.carousel');
    });


    // partners

    //$('.partnerCarousel').owlCarousel({
    //    rtl: true,
    //    margin: 15,
    //    smartSpeed: 1500,
    //    nav: false,
    //    navText: ['<i class="fa fa-angle-left"></i>', '<i class="fa fa-angle-right"></i>'],
    //    dots: false,
    //    loop: true,
    //    autoplay: true,
    //    mouseDrag: true,
    //    touchDrag: true,
    //    responsive: {
    //        0: {
    //            items: 1
    //        },
    //        600: {
    //            items: 1
    //        },
    //        1000: {
    //            items: 3
    //        }
    //    }
    //});
    // Custom Navigation Events

    $(".pBigNextBtn").click(function () {
        $('.partnerCarousel').trigger('next.owl.carousel');
    });
    $(".pBigPrevBtn").click(function () {
        $('.partnerCarousel').trigger('prev.owl.carousel');
    });


    // count number

    //$(".count").appear(function () {
    //    $(".count [data-to]").each(function () {
    //        var e = $(this).attr("data-to");
    //        $(this).delay(6e3).countTo({
    //            from: 50,
    //            to: e,
    //            speed: 3e3,
    //            refreshInterval: 50
    //        })
    //    })
    //});

    // accordion

    $('.accordion > .accordion_main:eq(0) .accordion_head').addClass('active').next().slideDown();
    $('.accordion_body').on('click', function (e) {
        e.stopPropagation();
    });
    $('.accordion .accordion_head').click(function (e) {
        var dropDown = $(this).closest('.accordion_main').find('.accordion_body');
        $(this).closest('.accordion').find('.accordion_body').not(dropDown).slideUp();
        if ($(this).hasClass('active')) {
            $(this).removeClass('active');
        } else {
            $(this).closest('.accordion').find('.accordion_head.active').removeClass('active');
            $(this).addClass('active');
        }
        dropDown.stop(false, true).slideToggle();
        e.preventDefault();
    });


    //tab

    $('.tab ul.tabs').addClass('active').find('> li:eq(0)').addClass('current');

    $('.tab ul.tabs li a').click(function (g) {
        var tab = $(this).closest('.tab'),
            index = $(this).closest('li').index();
        tab.find('ul.tabs > li').removeClass('current');
        $(this).closest('li').addClass('current');
        tab.find('.tab_content').find('div.tabs_item').not('div.tabs_item:eq(' + index + ')').slideUp();
        tab.find('.tab_content').find('div.tabs_item:eq(' + index + ')').slideDown();
        g.preventDefault();
    });


    // mix
    //if ($('.mixit_area').length) {
    //    var mixer = mixitup('.mixit_area', {
    //        selectors: {
    //            control: '[data-mixitup-control ]'
    //        },
    //        animation: {
    //            "duration": 550,
    //            "nudge": true,
    //            "reverseOut": true,
    //            "effects": "fade translateX(20%) translateY(20%) translateZ(22px) rotateX(90deg) rotateY(90deg) rotateZ(-18deg) stagger(41ms)"
    //        }
    //    });
    //    $("#filter-select").on("change", function () {
    //        $(".mixit_area").mixItUp('filter', this.value);
    //    });
    //}


    $(window).scroll(function () {
        if ($(this).scrollTop() > 100) {
            $('.scrollToTop').fadeIn();
        } else {
            $('.scrollToTop').fadeOut();
        }
    });

    //Click event to scroll to top

    var scrollToTop = $('#backTop');
    $(window).scroll(function () {
        if ($(this).scrollTop() >= 300) {
            scrollToTop.addClass('show');
        } else {
            scrollToTop.removeClass('show');
        }
    });
    scrollToTop.on('click', function () {
        $('html, body').animate({scrollTop: 0}, 800);
    });
});
