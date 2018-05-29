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
			else {
				switch (rule) {
					case "notZero":
						if (parseInt(value) === 0)
							errorMessage = "Pole jest wymagane.";
						break;
					case "mail":
						var re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
						if (!re.test(value.toLowerCase()))
							errorMessage = "Zły format adresu e-mail."
						break;
				}
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
		var _val = validateField(field);
		val = val === true ? _val : false;
	}.bind(this));
	return val;
}