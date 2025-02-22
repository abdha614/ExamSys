$(document).ready(function () {
    $('#CategoryId').change(function () {
        var categoryId = $(this).val();
        $.ajax({
            url: '/Professor/GetCoursesByCategory',
            type: 'GET',
            dataType: 'json',
            data: { categoryId: categoryId },
            success: function (data) {
                var courseSelect = $('#CourseId');
                courseSelect.empty();
                courseSelect.append('<option value="">Select Course</option>');
                $.each(data, function (index, course) {
                    courseSelect.append('<option value="' + course.id + '">' + course.name + '</option>');
                });
            },
            error: function (xhr, status, error) {
                console.log(error);
            }
        });
    });
});
