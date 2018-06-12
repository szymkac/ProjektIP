function openEditMeetingModal(id, details) {
	var _details = details ? details : false;
	$('#close-modal-button').html(_details ? "POWRÓT" : "ANULUJ");

	if (_details)
		$('#accept-modal-button').addClass('modal-button-unvisible');
	else
		$('#accept-modal-button').removeClass('modal-button-unvisible');

    $.ajax({
        type: "GET",
        url: "/Meeting/EditMeeting/",
		data: {
			Id: id,
			Details: _details
		},
		success: function (viewHTML) {
			if (_details)
				$('#modal-head').html('<h3>Szczegóły wydarzenia</h3>');
			else
				$('#modal-head').html('<h3>Edytuj wydarzenie</h3>');
            $('#modal-body').html(viewHTML);
            $('#modal-window').modal('show');
        },
        error: function (errorData) {
            console.log(errorData);
        }
    });
}

function confirmPrsent(id, confirmationOfPresence, column) {
	$.ajax({
		type: "POST",
		url: "/Meeting/PushChangeConfirmation/",
		data: {
			meetingId: id,
			confirmationofPresence: confirmationOfPresence
		},
		success: function (Json) {
			console.log("succes");
			if (Json)
				refreshMeetingColumn(column);
		},
		error: function (errorData) {
			console.log(errorData);
		}
	});
}