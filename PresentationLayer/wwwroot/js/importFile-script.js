
document.getElementById('importForm').addEventListener('submit', function (event) {
    var fileInput = document.getElementById('importFile');
    var messageArea = document.getElementById('messageArea');

    // Clear any previous messages
    messageArea.style.display = 'none';
    messageArea.innerHTML = '';

    if (!fileInput.files.length) {
        // Display error message if no file is selected
        messageArea.style.display = 'block';
        messageArea.innerHTML = 'No file selected. Please upload a CSV or DOCX file.';
        event.preventDefault();  // Prevent form submission if no file is selected
    }
});

// Hide the error message when a file is selected
document.getElementById('importFile').addEventListener('change', function () {
    var messageArea = document.getElementById('messageArea');
    messageArea.style.display = 'none'; // Hide message when a file is chosen
});

function downloadTemplate(templateType, fileExtension) {
    const url = `/Professor/DownloadTemplate?templateType=${templateType}&fileExtension=${fileExtension}`;
    window.location.href = url; // Navigate to the generated URL
}