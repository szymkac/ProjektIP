function openModal(url) {
	$.ajax({
		type: "GET",
		url: url,
		success: function (viewHTML) {
			$('#modal-body').html(viewHTML);
			$('#modal-window').modal('show');
		},
		error: function (errorData) {
			console.log(errorData);
		}
	});
}
function openModalAddUser() {
	$('#modal-head').html('<h3>Dodaj pracownika</h3>');
	this.openModal("/Employee/AddEmployee/");
}
function openModalChoosenUsers() {
	mode = 3;
	$('#modal-head').html('<h3>Wybierz pracowników</h3>');
	this.openModal("/Employee/ChooseEmployee/");
}

function openModalTaskManager() {
	$('#modal-head').html('<h3>Zarządzanie zadaniami</h3>');
	this.openModal("/Task/TaskManager/");
}
function openModalAddTask() {
	$('#modal-head').html('<h3>Dodaj zadanie</h3>');
	this.openModal("/Task/AddTask/");
}
function openModalAddMeeting() {
	$('#modal-head').html('<h3>Dodaj spotkanie</h3>');
	this.openModal("/Meeting/AddMeeting/");
}
function closeModal() {
	$('#modal-window').modal('hide');
}
function saveModal() {
	var val = validateForm();
	if (val)
		$("#partialViewForm").submit();
}