$(function () {
    $("#btnsea").click(function () {
        window.location.href = "/Home/Search?key=" + $("#key").val();
    })
    $("#cancel").click(function () {
        $("#weixin").css("display", "none");
        $("#cancel").css("display", "none");
    })
});
//轮播图效果
$(document).ready(function () {
    $('#s').focusin(function () {
        var val = $(this).val();
        if (val == '请输入搜索内容...') {
            $(this).val('');
        }
    });

    $('#s').focusout(function () {
        var val = $(this).val();
        if (val == '') {
            $(this).val('请输入搜索内容...');
        }
    });

    $('#weeknews_triger').click(function () {
        $('#weeknews').animate({
            height: '293'
        });
        $('#medianews').animate({
            height: '40'
        });
    });

    $('#medianews_triger').click(function () {
        $('#weeknews').animate({
            height: '40'
        });
        $('#medianews').animate({
            height: '293'
        });
    });

    $('.link2home').hover(function () {
        $(this).css('background-position', '0 -30px');
        $('body').animate({ backgroundPosition: '0 0' });
        $('#header_top_nav a').hide();
        $('#header_top_nav a.more').show();
        $('#header_top_nav').animate({ height: 20 });
    });

    $('.imp_news').hover(function () {
        $('body').animate({ backgroundPosition: '0 0' });
        $('#header_top_nav a').hide();
        $('#header_top_nav a.inews').show();
        $('#header_top_nav').animate({ height: 20 });
    });

    $('.camp_news').hover(function () {
        $('body').animate({ backgroundPosition: '0 0' });
        $('#header_top_nav a').hide();
        $('#header_top_nav a.cnews').show();
        $('#header_top_nav').animate({ height: 20 });
    });

    $('#slides').slides({
        preload: true,
        preloadImage: '/Content/images/slide/ajax-loader.gif',
        effect: 'slide',
        play: 5000,
        pause: 2000,
        hoverPause: true,
        animationStart: function (current) {
            $('.caption').animate({
                bottom: 0,
                right: 40,
                opacity: 0
            }, 200);
        },
        animationComplete: function (current) {
            $('.caption').animate({
                bottom: 20,
                right: 40,
                opacity: 1
            }, 200);
        },
        slidesLoaded: function () {
            $('.caption').animate({
                bottom: 20,
                right: 40,
                opacity: 1
            }, 200);
        }
    });

    $('#wrapper').bind("mouseleave", function () {
        $('#header_top_nav').animate({ height: 0 });
        $('body').animate({ backgroundPosition: '0 -20px' });
    });
});

//图片滚动效果
$(document).ready(function (e) {
    /***不需要自动滚动，去掉即可***/
    time = window.setInterval(function () {
        $('.og_next').click();
    }, 5000);
    /***不需要自动滚动，去掉即可***/
    linum = $('.mainlist li').length;//图片数量
    w = linum * 250;//ul宽度
    $('.piclist').css('width', w + 'px');//ul宽度
    $('.swaplist').html($('.mainlist').html());//复制内容
    $('.og_next').click(function () {
        if ($('.swaplist,.mainlist').is(':animated')) {
            $('.swaplist,.mainlist').stop(true, true);
        }
        if ($('.mainlist li').length > 4) {//多于4张图片
            ml = parseInt($('.mainlist').css('left'));//默认图片ul位置
            sl = parseInt($('.swaplist').css('left'));//交换图片ul位置
            if (ml <= 0 && ml > w * -1) {//默认图片显示时
                $('.swaplist').css({ left: '1000px' });//交换图片放在显示区域右侧
                $('.mainlist').animate({ left: ml - 500 + 'px' }, 'slow');//默认图片滚动
                if (ml == (w - 1000) * -1) {//默认图片最后一屏时
                    $('.swaplist').animate({ left: '0px' }, 'slow');//交换图片滚动
                }
            } else {//交换图片显示时
                $('.mainlist').css({ left: '1000px' })//默认图片放在显示区域右
                $('.swaplist').animate({ left: sl - 1000 + 'px' }, 'slow');//交换图片滚动
                if (sl == (w - 1000) * -1) {//交换图片最后一屏时
                    $('.mainlist').animate({ left: '0px' }, 'slow');//默认图片滚动
                }
            }
        }
    })
    $('.og_prev').click(function () {

        if ($('.swaplist,.mainlist').is(':animated')) {
            $('.swaplist,.mainlist').stop(true, true);
        }
        if ($('.mainlist li').length > 4) {
            ml = parseInt($('.mainlist').css('left'));
            sl = parseInt($('.swaplist').css('left'));
            if (ml <= 0 && ml > w * -1) {
                $('.swaplist').css({ left: w * -1 + 'px' });
                $('.mainlist').animate({ left: ml + 1000 + 'px' }, 'slow');
                if (ml == 0) {
                    $('.swaplist').animate({ left: (w - 1000) * -1 + 'px' }, 'slow');
                }
            } else {
                $('.mainlist').css({ left: (w - 1000) * -1 + 'px' });
                $('.swaplist').animate({ left: sl + 1000 + 'px' }, 'slow');
                if (sl == 0) {
                    $('.mainlist').animate({ left: '0px' }, 'slow');
                }
            }
        }
    })
});
$(document).ready(function () {
    $('.og_prev,.og_next').hover(function () {
        $(this).fadeTo('fast', 1);
    }, function () {
        $(this).fadeTo('fast', 0.7);
    })
})
