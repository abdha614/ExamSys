//$(document).ready(function () {
//    $('#CategoryId').change(function () {
//        var categoryId = $(this).val();
//        $.ajax({
//            url: '/Professor/GetCoursesByCategory',
//            type: 'GET',
//            dataType: 'json',
//            data: { categoryId: categoryId },
//            success: function (data) {
//                var courseSelect = $('#CourseId');
//                courseSelect.empty();
//                courseSelect.append('<option value="">Select Course</option>');
//                $.each(data, function (index, course) {
//                    courseSelect.append('<option value="' + course.id + '">' + course.name + '</option>');
//                });
//            },
//            error: function (xhr, status, error) {
//                console.log(error);
//            }
//        });
//    });
//});




//////////////
// Get DOM elements
const spinnerInput = document.getElementById('lectureSpinner');
const hiddenLecture = document.getElementById('lectureHidden');

// Extract current lecture number from the hidden input (if it exists)
let currentLecture = 1; // Default value
if (hiddenLecture.value) {
    // Extract the number from "Lecture X" format (e.g., "Lecture 3" → 3)
    const match = hiddenLecture.value.match(/Lecture (\d+)/);
    if (match && match[1]) {
        currentLecture = parseInt(match[1]);
    }
}

const minLecture = 1;    // Minimum lecture number
const maxLecture = 20;   // Maximum number of lectures

// Update the displayed lecture name and hidden field
function updateLectureDisplay() {
    const formattedValue = `Lecture ${currentLecture}`;
    spinnerInput.value = formattedValue;
    hiddenLecture.value = formattedValue;
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