﻿$(function () { $('#addShoucang').click(function () { var goodId = $('#GoodId').val(); var userid = $('#UserId').val(); if (userid == "") { if (confirm("您还没有登录，是否立即登录")) { window.location.href = "/Account/Login?oldurl=/Home/ShowGoods/" + goodId } } $.post('/home/AddShouCang', { userId: userid, goodId: goodId }, function (data) { alert(data) }) }) })