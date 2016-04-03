
function uploadimg() {
    if ($("#img_file").val().length > 0) {
        //alert($("#img_file").val().length);
        ajaxFileUpload();
    }
    else {
        alert("请选择图片");
    }
}
function ajaxFileUpload() {
    $.ajaxFileUpload
    (
        {
            url: '/ashx/Upload.ashx', //用于文件上传的服务器端请求地址
            secureuri: false, //一般设置为false
            fileElementId: 'img_file', //文件上传空间的id属性  <input type="file" id="file" name="file" />
            dataType: 'text', //返回值类型 一般设置为json
            success: function (data, status)  //服务器成功响应处理函数
            {
                var msgs = data.split('>');
                var splits = msgs[1].split('<');

                if (splits[0] == "1") {
                    alert("图片大小不要大于500kb哦");
                    return;
                }

                var attr = data.split('|');
                var msg = attr[0].split('>');

                $("#imsges_my").css("display", "block");
                $("#imsges_my").attr("src", msg[1]);
                $("#bigimage").val(msg[1]);
                $('#touxiang').val(msg[1]);
                if (typeof (data.error) != 'undefined') {
                    if (data.error != '') {
                        alert(data.error);
                    } else {
                        alert(data.msg);
                    }
                }
            },
            error: function (data, status, e)//服务器响应失败处理函数
            {
                alert(e);
            }
        }
    )
    return false;
}
