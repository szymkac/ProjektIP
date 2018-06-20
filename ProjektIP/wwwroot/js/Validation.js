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
                    case "numbers":
                        var re =/\(?([0-9]{3})\)?([ .-]?)([0-9]{3})\2([0-9]{3})/;
                        if (!re.test(value.toLowerCase()))
                            errorMessage = "Zły format numeru telefonu."
                        break;
                    case "date":
                        var re = /^(?:(?:31(\.)(?:0?[13578]|1[02]))\1|(?:(?:29|30)(\.)(?:0?[1,3-9]|1[0-2])\2))(?:(?:1[6-9]|[2-9]\d)?\d{2})$|^(?:29(\.)0?2\3(?:(?:(?:1[6-9]|[2-9]\d)?(?:0[48]|[2468][048]|[13579][26])|(?:(?:16|[2468][048]|[3579][26])00))))$|^(?:0?[1-9]|1\d|2[0-8])(\/|-|\.)(?:(?:0?[1-9])|(?:1[0-2]))\4(?:(?:1[6-9]|[2-9]\d)?\d{2})$/;
                        if (!re.test(value.toLowerCase()))
                            errorMessage = "Zły format daty."
                        break;
                    case "time":
                        var re = /([01]?[0-9]|2[0-3]):[0-5][0-9]/;
                        if (!re.test(value.toLowerCase()))
                            errorMessage = "Zły format godziny."
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