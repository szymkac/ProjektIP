function getDate(myDate) {
	var dd = myDate.getDate();
	var mm = myDate.getMonth() + 1;
	var yyyy = myDate.getFullYear();

	if (dd < 10) {
		dd = '0' + dd;
	}

	if (mm < 10) {
		mm = '0' + mm;
	}

	var date = mm + '/' + dd + '/' + yyyy;
	return date;
}
function dateIncrease(date, plusDays) {
	var myDate = new Date(date);
	myDate.setDate(myDate.getDate() + plusDays);
	return getDate(myDate);
}
function dateDecrease(date, minusDays) {
	var myDate = new Date(date);
	myDate.setDate(myDate.getDate() - minusDays);
	return getDate(myDate);
}
function changeDateFormat(date, separator) {
	var _separator = separator ? separator : '/';
	myDate = date.split('/');
	return myDate[1] + _separator + myDate[0] + _separator + myDate[2];
}