var XCLNetSearch_CommonJs = {
    //设置options选中,obj:js对象
    SelectedObj: function (obj, v) {
        $(obj).find("option").removeAttr("selected").filter("[value='" + v + "']").prop({ "selected": true });
    }
};