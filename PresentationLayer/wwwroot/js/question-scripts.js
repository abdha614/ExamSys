function checkNewCategory(value) {
    if (value === 'new') {
        $('#newCategoryDiv').show();
    } else {
        $('#newCategoryDiv').hide();
        getCoursesByCategory(value);
    }
}

// Function to add a new category
// Function to add a new category
// Function to add a new category
function addCategory() {
    var categoryName = $('#newCategoryName').val().trim(); // Get the value and trim extra spaces

    // Check if the category name is not empty
    if (categoryName === "") {
        $('#categoryNotification').hide(); // Hide notification if input is empty
        return; // Do nothing if the input is empty
    }

    // Check if the category already exists in the dropdown
    var categoryExists = false;
    $('#category option').each(function () {
        if ($(this).text().trim().toLowerCase() === categoryName.toLowerCase()) {
            categoryExists = true;
            return false; // Break out of the loop
        }
    });

    // If the category exists, show the notification
    if (categoryExists) {
        $('#categoryNotification').text('Category already exists. Please choose a different name or select it from the list.').show(); // Show the notification
        return; // Do nothing if the category already exists
    } else {
        $('#categoryNotification').hide(); // Hide the notification if the input is valid
    }

    // If the category doesn't exist, proceed with the AJAX request
    $.ajax({
        url: '/Professor/AddCategory',
        type: 'POST',
        data: { name: categoryName },
        success: function (data) {
            // Add the new category to the dropdown list
            $('#category').append('<option value="' + data.id + '">' + data.name + '</option>');

            // Select the new category
            $('#category').val(data.id);

            // Hide the "Add New Category" input field
            $('#newCategoryDiv').hide();

            // Optionally, load courses for the newly added category
            getCoursesByCategory(data.id);

            // Hide the notification after successful addition
            $('#categoryNotification').hide();
        },
        error: function () {
            // Handle any errors (optional)
            $('#categoryNotification').text('An error occurred while adding the category.').show();
        }
    });
}

// Event listener for category selection
$('#category').change(function () {
    // Hide the notification whenever the dropdown value changes
    $('#categoryNotification').hide();

    // Show the "Add New Category" form only if "new" is selected
    if ($(this).val() === 'new') {
        $('#newCategoryDiv').show();
    } else {
        $('#newCategoryDiv').hide();
    }
});

// Event listener to hide the notification when the user starts typing or focuses the input field again
$('#newCategoryName').on('input', function () {
    $('#categoryNotification').hide(); // Hide the notification if the user starts typing a new category
});


// Function to add a new course
// Function to add a new course
// Event listener for course selection
$('#course').change(function () {
    $('#courseNotification').hide(); // Hide the notification whenever the dropdown value changes

    // Show the "Add New Course" form only if "new" is selected
    if ($(this).val() === 'new') {
        $('#newCourseDiv').show();
    } else {
        $('#newCourseDiv').hide();
    }
});

function addCourse() {
    var courseName = $('#newCourseName').val().trim(); // Get the value and trim extra spaces
    var categoryId = $('#category').val(); // Get selected category ID

    // Check if the course name is not empty
    if (courseName === "") {
        $('#courseNotification').text("Please enter a course name.").show(); // Show error message for empty input
        return; // Do nothing if the input is empty
    }

    // Check if a category is selected
    if (categoryId === "" || categoryId === "new") {
        $('#courseNotification').text("Please select a valid category before adding a course.").show();
        return;
    }

    // Check if the course already exists in the dropdown
    var courseExists = false;
    $('#course option').each(function () {
        if ($(this).text().trim().toLowerCase() === courseName.toLowerCase()) {
            courseExists = true;
            return false; // Break out of the loop
        }
    });

    // If the course exists, show the notification
    if (courseExists) {
        $('#courseNotification').text('Course already exists. Please choose a different name or select it from the list.').show(); // Show the notification
        return; // Do nothing if the course already exists
    } else {
        $('#courseNotification').hide(); // Hide the notification if the input is valid
    }

    // Send AJAX request to the backend
    $.ajax({
        url: '/Professor/AddCourse',
        type: 'POST',
        data: { name: courseName, categoryId: categoryId },
        success: function (response) {
            if (response.error) {
                // Show error message from backend
                $('#courseNotification').text(response.errorMessage).show();
            } else {
                // Add the new course to the dropdown list
                $('#course').append('<option value="' + response.data.id + '">' + response.data.name + '</option>');
                $('#course').val(response.data.id); // Set the new course as selected
                $('#newCourseDiv').hide(); // Hide the "Add New Course" input field
                $('#courseNotification').hide(); // Hide any notifications
            }
        },
        error: function () {
            // Handle any errors
            $('#courseNotification').text('An error occurred while adding the course.').show();
        }
    });
}


$('#newCourseName').on('input', function () {
    $('#courseNotification').hide(); // Hide notification when user types a new course
});


function getCoursesByCategory(categoryId) {
    $.ajax({
        url: '/Professor/GetCoursesByCategory',
        type: 'GET',
        data: { categoryId: categoryId },
        success: function (data) {
            var courseDropdown = $('#course');
            courseDropdown.empty();
            courseDropdown.append('<option value="">Select Course</option>');
            $.each(data, function (i, course) {
                courseDropdown.append('<option value="' + course.id + '">' + course.name + '</option>');
            });
            courseDropdown.append('<option value="new">Add New Course</option>');
        }
    });
}

$(document).ready(function () {
    $('#category').change(function () {
        checkNewCategory($(this).val());
    });

    $('#course').change(function () {
        if ($(this).val() === 'new') {
            $('#newCourseDiv').show();
        } else {
            $('#newCourseDiv').hide();
        }
    });

    // Load courses if category is already selected
    var selectedCategoryId = $('#category').val();
    if (selectedCategoryId && selectedCategoryId !== 'new') {
        getCoursesByCategory(selectedCategoryId);
    }

});

$(document).ready(function () {
    // Add Answer
    $('#addAnswer').click(function () {
        var index = $('.answer-group').length; // Get the current count of answer groups
        var newAnswer = `
            <div class="input-group mb-2 answer-group align-items-center">
                <!-- Input for Answer Text -->
                <input name="Answers[${index}].Text" class="form-control" data-val="true" data-val-required="Answer text is required." />

                <!-- Correct Option -->
                <div class="input-group-append">
                    <div class="input-group-text d-flex justify-content-center align-items-center">
                        <input type="radio" name="CorrectAnswerIndex" value="${index}" />
                    </div>
                </div>

                <!-- Remove Button -->
                <button type="button" class="btn btn-danger remove-answer d-flex align-items-center" style="font-size: 20px; padding: 5px 10px; height: 100%;">
                    Remove
                </button>
            </div>
            <span class="text-danger" data-valmsg-for="Answers[${index}].Text" data-valmsg-replace="true"></span>
        `;
        $('#answersContainer').append(newAnswer);

        // Reinitialize validation to include new inputs
        $.validator.unobtrusive.parse($('#answersContainer'));
    });

    // Remove Answer
    $(document).on('click', '.remove-answer', function () {
        var totalAnswers = $('.answer-group').length;
        if (totalAnswers > 3) {
            // Remove the answer and its associated error message
            $(this).closest('.answer-group').next('span').remove();
            $(this).closest('.answer-group').remove();

            // Re-index remaining answers
            $('.answer-group').each(function (i) {
                $(this).find('input[name^="Answers"]').attr('name', `Answers[${i}].Text`);
                $(this).find('input[type="radio"]').attr('value', i);
                $(this).next('span').attr('data-valmsg-for', `Answers[${i}].Text`);
            });

            $('#minAnswerNotification').hide(); // Hide notification for minimum answers
        } else {
            $('#minAnswerNotification').text('You must have at least 3 options.').show();
        }
    });

    // Real-time validation for Answer Text
    $(document).on('input', '.answer-group input[name^="Answers"]', function () {
        var inputGroup = $(this).closest('.answer-group');
        var textInput = inputGroup.find('input[name^="Answers"]').val().trim();

        if (!textInput) {
            inputGroup.next('span').text('Please enter a Answer text.').show();
        } else {
            inputGroup.next('span').text('').hide();
        }
    });

    // Validate Form Before Submitting
    $('form').submit(function (e) {
        var isValid = true;

        $('.answer-group').each(function () {
            var inputGroup = $(this);
            var textInput = inputGroup.find('input[name^="Answers"]').val().trim();

            if (!textInput) {
                inputGroup.next('span').text('Please enter a Answer text.').show();
                isValid = false;
            } else {
                inputGroup.next('span').text('').hide();
            }
        });

        if (!isValid) {
            e.preventDefault(); // Prevent form submission if any validation fails
        }
    });
});
