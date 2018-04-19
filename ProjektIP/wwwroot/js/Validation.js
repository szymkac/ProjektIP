function validateField(field) {
	var value = field.value;
	var validationRules = field.dataset.validationrules.split(',');
	var errorMessage = "";


	if (value !== "" && value !== null) {
		$.each(validationRules, function (i, rule) {
			if (rule.includes("maxLength")) {
				var maxLength = parseInt(rule.replace("maxLength", ""));
				if (value.length > maxLength)
					errorMessage += "Przekroczono maksymalną liczbę znaków (" + maxLength + "). ";
			}

		}.bind(this));
	}
	else if (validationRules.indexOf("required") !== -1) {
		errorMessage = "Pole jest wymagane.";
	}

	$('[data-validator="' + field.name + '"]').text(errorMessage);
	return errorMessage === "" ? true : false;
}
function validateForm() {
	var val = true;
	var fields = $('[data-validationRules]');
	$.each(fields, function (i, field) {
		val = val === true ? validateField(field) : false;
	}.bind(this));
	return val;
}