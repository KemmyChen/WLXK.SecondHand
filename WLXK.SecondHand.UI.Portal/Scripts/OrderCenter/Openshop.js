﻿$(function () { $("#imsges_my").css("display", "none"); $("#getcode").click(function () { $.post("/Others/GetCodeByEmail", { id: $("#userid").text() }, function (data) { alert(data) }) }); $("#txtcode").blur(function () { $.post("/Others/CompareCode", { id: $("#userid").text(), code: $("#txtcode").val() }, function (data) { var message = $("#mseeage"); if (data == "1") { message.css("color", "green"); message.text("验证成功"); $("#IsCode").val("1") } else { message.css("color", "red"); message.text("验证码输入有误"); $("#IsCode").val("0") } }) }); $("#txtKouHao").blur(function () { $.post("/Others/CompareKouHao", { code: $("#txtKouHao").val() }, function (data) { var message = $("#kouhaomes"); if (data == "ok") { message.css("color", "green"); message.text("验证成功"); $("#IsKouHao").val("1") } else { message.css("color", "red"); message.text("口号输入有误"); $("#IsKouHao").val("0") } }) }); $("#btnup").click(function () { if ($('#img_file').val() == "") { alert("请选择您的头像图片"); return } $("#form1").ajaxSubmit({ url: "/ashx/Upload.ashx", type: "Post", success: function (data) { var split = data.split('|'); if (data == "error") { alert("错误了") } else { $("#imsges_my").css("display", "block"); $('#imsges_my').attr('src', split[0]); $('#CardAddress').val(split[0]) } } }) }) })