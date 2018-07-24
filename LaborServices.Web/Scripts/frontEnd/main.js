/*global $, jQuery*/

$(document).ready(function () {

    'use strict';
    $('.navbar .nav-item a').click(function () {
        $(this).children('.navbar .dropdown-toggle i').toggleClass('fa-angle-double-left');
        $(this).children('.navbar .dropdown-toggle i').toggleClass('fa-angle-double-down');
    });

    $('.navbar .dropdown-menu .subMenu').click(function () {
        $(this).siblings('.list').slideToggle();
        $(this).children('i').eq(1).toggleClass('fa fa-angle-down');
        $(this).children('i').eq(1).toggleClass('fa fa-angle-up');
        return false;
    });

    $('.profile .c1 h3 i').click(function () {
        $(this).toggleClass('fa fa-plus');
        $(this).toggleClass('fa fa-minus');
        $('.profile .c1 ul').slideToggle();
    });

    $('.profile .c2 h3 i').click(function () {
        $(this).toggleClass('fa fa-plus');
        $(this).toggleClass('fa fa-minus');
        $('.profile .c2 ul').slideToggle();
    });

    $('.order .mainUl li').on('click', function () {
        $(this).children('.subUl').toggleClass('active');
        $(this).siblings().children('.subUl').removeClass('active');
    });

    $('.order .mainUl li .subUl li input').on('click', function () {
        if ($(this).attr('type') === 'radio') {
            return true;
        } else {
            return false;
        }
    });


    'use strict';

    $('#new').click(function () {
        $('#choose').slideToggle();
        $('#new i').eq(1).toggleClass('fa fa-angle-down');
        $('#new i').eq(1).toggleClass('fa fa-angle-up');
        return false;
    });


    $(document).ajaxStart(function () {
        fakeLoaderFadeIn();
    });

    $(document).ajaxStop(function () {
        fakeLoaderFadeOut();
    });
    //$('html').niceScroll({
    //    cursorwidth: '5px',
    //    cursorcolor: '#5eb3e4',
    //    cursorborder: 'none'
    //});


    //	Other Team Js Code
    $(function () {
        //adjust sider height
        var winH = $(window).height(),
            upperH = $('.upper-bar').innerHeight(),
            navH = $('.navbar').innerHeight();
        $('.slider').height(winH - (upperH + navH));

        $('.panel-scroll').perfectScrollbar({
            wheelSpeed: 50,
            minScrollbarLength: 20,
            suppressScrollX: true
        });


    });

});



