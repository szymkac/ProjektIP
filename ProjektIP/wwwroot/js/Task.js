
function changeStatus(id, newStatus) {
    $.ajax({
        type: "POST",
        url: "/Task/ChangeStatus/",
        data: {
            id: id,
            status: newStatus,
            date: getDate(new Date())
        }
    });
    getTasks();
    getTasks();
}
function getDate(myDate) {
    var dd = myDate.getDate();
    var mm = myDate.getMonth() + 1;
    var yyyy = myDate.getFullYear();

    if (dd < 10) {
        dd = '0' + dd
    }

    if (mm < 10) {
        mm = '0' + mm
    }

    var date = mm + '/' + dd + '/' + yyyy;
    return date;
}