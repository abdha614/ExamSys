$(document).ready(function () {
    // Trigger the course loading if a category is already selected on page load
    var selectedCategoryId = $('#category').val();
    var selectedCourseId = $('#course').data('selected-course-id');
    if (selectedCategoryId) {
        getCoursesByCategory(selectedCategoryId, selectedCourseId);
    }

    // Handle the change event of category dropdown
    $('#category').change(function () {
        var categoryId = $(this).val();
        getCoursesByCategory(categoryId);
    });
});

function getCoursesByCategory(categoryId, selectedCourseId = null) {
    $.ajax({
        url: '/Professor/GetCoursesByCategory',
        type: 'GET',
        data: { categoryId: categoryId },
        success: function (data) {
            var courseDropdown = $('#course');
            courseDropdown.empty();
            courseDropdown.append('<option value="">All</option>');
            $.each(data, function (i, course) {
                courseDropdown.append('<option value="' + course.id + '">' + course.name + '</option>');
            });
            if (selectedCourseId) {
                courseDropdown.val(selectedCourseId);
            }
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
   