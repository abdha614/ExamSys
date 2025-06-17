

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

    function loadCourses(categoryId) {
        if (!categoryId) {
        $('#course').empty().append('<option value="">Select Course</option>');
    return;
        }

    $.ajax({
        url: '/Professor/GetCoursesByCategory', // Replace with your actual controller route
    type: 'GET',
    data: {categoryId: categoryId },
    success: function (courses) {
                var courseDropdown = $('#course');
        courseDropdown.empty();
        courseDropdown.append('<option value="" disabled selected>-- Select Course --</option>');
    //courseDropdown.append('<option value="">Select Course</option>');


    $.each(courses, function (index, course) {
        courseDropdown.append('<option value="' + course.id + '">' + course.name + '</option>');
                });
            },
    error: function () {
        alert('Error loading courses');
            }
        });
    }


////////////////////
let currentLecture = 1;  // Starting index for the lecture
const minLecture = 1;    // Minimum lecture number
const maxLecture = 20;   // Maximum number of lectures

const spinnerInput = document.getElementById('lectureSpinner');
const hiddenLecture = document.getElementById('lectureHidden');

// Update the displayed lecture name and hidden field
function updateLectureDisplay() {
    spinnerInput.value = `Lecture ${currentLecture}`; // Format as "Lecture X"
    hiddenLecture.value = `Lecture ${currentLecture}`; // Update hidden field to include "Lecture"
}

// Increment the lecture index (if within bounds)
function incrementLecture() {
    if (currentLecture < maxLecture) {
        currentLecture++;
        updateLectureDisplay();
    }
}

// Decrement the lecture index (if within bounds)
function decrementLecture() {
    if (currentLecture > minLecture) {
        currentLecture--;
        updateLectureDisplay();
    }
}

// Initialize the display when the page loads
updateLectureDisplay();