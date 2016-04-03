$(function () {
    $("#suolvetu").css("display", "none");
    //$("#btnUpload").click(function () {
    //    if ($("#fileupload").val() != "") {
    //        $("#od_bd form").ajaxSubmit({
    //            url: "/ashx/upload.ashx",
    //            type: "post",
    //            success: function (data) {
    //                if (data == "error") {
    //                    alert("错误了")
    //                } else {
    //                    $("#suolvetu").css("display", "block");
    //                    $("#suolvetu1").css("display", "none");
    //                    var image = data.split("|");
    //                    $("#bigimage").val(image[0]);
    //                    $("#smallimage").val(image[1]);
    //                    $("#suolvetu").attr("src", image[1])
    //                }
    //            }
    //        })
    //    } else {
    //        alert("请选择图片文件进行上传")
    //    }
    //});
    $("#form0").validate({
        rules: {
            Type: {
                required: true,
            },
            Title: {
                required: true,
                maxlength: 40,
                minlength: 10
            },
            MaiDian: {
                required: true,
                maxlength: 256,
                minlength: 20
            },
            Price: {
                required: true,
                number: true
            },
            Count: {
                required: true,
                number: true
            },
            LaiYuan: {
                required: true,
                minlength: 10,
                maxlength: 60
            },
            SmallImageAddress: {
                required: true,
            },
            BigImageAddress: {
                required: true,
            },
            BigImageAddress: {
                required: true,
            },
            Descript: {
                required: true,
                minlength: 10,
                maxlength: 2000
            },
            RealName: {
                required: true,
            },
            QQ: {
                required: true,
                number: true
            },
            PhoneNum: {
                required: true,
                number: true
            },
            Address: {
                required: true,
            }
        },
        messages: {
            Type: {
                required: "<span class='error'>*请选择宝贝类型</span>",
            },
            Title: {
                required: "<span class='error'>*请输入标题</span>",
                maxlength: "<span class='error'>*商品标题不得多于40个字</span>",
                minlength: "<span class='error'>*商品标题不得少于10个字</span>"
            },
            MaiDian: {
                required: "<span class='error'>*请输入卖点</span>",
                maxlength: "<span class='error'>*商品卖点不得多于250个字</span>",
                minlength: "<span class='error'>*商品卖点不得少于20个字</span>"
            },
            Price: {
                required: "<span class='error'>*请输入价格</span>",
                number: "<span class='error'>*请正确输入商品价格</span>"
            },
            Count: {
                required: "<span class='error'>*请输入商品数量</span>",
                number: "<span class='error'>*请正确输入商品数量</span>"
            },
            LaiYuan: {
                required: "<span class='error'>*请输入商品来源</span>",
                minlength: "<span class='error'>*商品来源不得少于10个汉字</span>",
                maxlength: "<span class='error'>*商品来源不得多于60个汉字</span>"
            },
            SmallImageAddress: {
                required: "<span class='error'>*请上传商品图片</span>",
            },
            BigImageAddress: {
                required: "<span class='error'>*请上传商品图片</span>",
            },
            Descript: {
                required: "<span class='error'>*请输入商品描述</span>",
                minlength: "<span class='error'>*商品描述不得少于10字</span>",
                minlength: "<span class='error'>*商品描述不得多于2000字</span>",
            },
            RealName: {
                required: "<span class='error'>*请输入真实姓名</span>"
            },
            QQ: {
                required: "<span class='error'>*请输入联系QQ</span>",
                number: "<span class='error'>*请输入正确的QQ号码</span>"
            },
            PhoneNum: {
                required: "<span class='error'>*请输入联系方式</span>",
                number: "<span class='error'>*请输入正确的联系方式</span>"
            },
            Address: {
                required: "<span class='error'>*请输入您的地址</span>",
            }
        }
    })
})