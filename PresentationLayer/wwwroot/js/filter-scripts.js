$(document).ready(function () {
    // Trigger the course loading if a category is already selected on page load
    var selectedCategoryId = $('#category').val();
    var selectedCourseId = $('#course').data('selected-course-id');
    var selectedLectureId = $('#lecture').data('selected-lecture-id');

    if (selectedCategoryId) {
        getCoursesByCategory(selectedCategoryId, selectedCourseId);
    }
    // Trigger lecture loading if a course is already selected on page load
    if (selectedCourseId) {
        getLecturesByCourse(selectedCourseId, selectedLectureId);
    }
    // Handle the change event of category dropdown
    $('#category').change(function () {
        var categoryId = $(this).val();
        getCoursesByCategory(categoryId);
    });
    // Handle the change event of course dropdown
    $('#course').change(function () {
        var courseId = $(this).val();
        getLecturesByCourse(courseId);
    });
});

function getCoursesByCategory(categoryId, selectedCourseId = null) {
    $.ajax({
        url: '/Professor/GetCoursesByyCategory',
        type: 'GET',
        data: { categoryId: categoryId },
        success: function (data) {
            var courseDropdown = $('#course');
            courseDropdown.empty();
            courseDropdown.append('<option value="">All Courses</option>');
            $.each(data, function (i, course) {
                courseDropdown.append('<option value="' + course.id + '">' + course.name + '</option>');
            });
            if (selectedCourseId) {
                courseDropdown.val(selectedCourseId);
            }
        }
    });
}


function getLecturesByCourse(courseId, selectedLectureId = null) {
    $.ajax({
        url: '/Professor/GetLecturesByCourse',
        type: 'GET',
        data: { courseId: courseId },
        success: function (data) {
            var lectureDropdown = $('#lecture');
            lectureDropdown.empty();
            lectureDropdown.append('<option value="">All Lectures</option>');

            // ✅ Sort by numeric part of lectureName
            data.sort(function (a, b) {
                const numA = parseInt(a.lectureName.match(/\d+/));
                const numB = parseInt(b.lectureName.match(/\d+/));
                return numA - numB;
            });

            // Append sorted lectures
            $.each(data, function (i, lecture) {
                lectureDropdown.append('<option value="' + lecture.id + '">' + lecture.lectureName + '</option>');
            });

            // Pre-select if needed
            if (selectedLectureId) {
                lectureDropdown.val(selectedLectureId);
            }
        },
        error: function (xhr, status, error) {
            console.error('Error fetching lectures:', error);
        }
    });
}


// Handle delete confirmation
$('body').on('click', '.btn-delete-question', function () {
    var questionId = $(this).data('question-id');
    const deleteForm = document.getElementById('deleteForm');
    deleteForm.action = `/Professor/DeleteQuestion/${questionId}`; // Adjust if the controller name differs
    const deleteModal = new bootstrap.Modal(document.getElementById('deleteModal'));
    deleteModal.show();
});

$(document).ready(function () {
    $('#applyFilters').on('click', function () {
        var questionTypeId = $('#questionType').val();
        var difficultyLevelId = $('#difficultyLevel').val();
        var categoryId = $('#category').val();
        var courseId = $('#course').val();
        var lectureId = $('#lecture').val();

        $.ajax({
            url: '/Professor/FilterQuestions', 
            type: 'GET',
            data: {
                questionTypeId: questionTypeId,
                difficultyLevelId: difficultyLevelId,
                categoryId: categoryId,
                courseId: courseId,
                lectureId: lectureId
            },
            success: function (result) {
                $('#questionsTableContainer').html(result);
            },
            error: function () {
                alert("An error occurred while filtering questions.");
            }
        });
    });
});