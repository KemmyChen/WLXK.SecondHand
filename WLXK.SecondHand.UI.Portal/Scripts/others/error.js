﻿$(function () { var i = 5; setInterval(function () { i--; $('#second').text(i); if (i <= 0) { window.location.href = $('#linkto').attr('href') } }, 1000) })