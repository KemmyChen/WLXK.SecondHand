﻿$(function () { $(".delete").click(function () { var id = $(this).attr("ids"); if (confirm("确认要还原本条商品吗？")) { $.post("/OrderCenter/RemeoveDraw", { id: id }, function (data) { if (data == "ok") { alert("商品还原成功") } else { alert("data") } }) } }) })