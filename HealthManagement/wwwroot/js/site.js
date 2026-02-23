// Custom JavaScript for Health Management System

$(document).ready(function () {
    // Auto-hide alerts after 5 seconds
    setTimeout(function () {
        $('.alert').fadeOut('slow');
    }, 5000);

    // Confirm delete actions
    $('[data-confirm]').on('click', function (e) {
        if (!confirm($(this).data('confirm'))) {
            e.preventDefault();
        }
    });

    // Client-side validation enhancement
    $('form').on('submit', function () {
        if ($(this).valid()) {
            $(this).find('[type="submit"]').prop('disabled', true).html('<span class="spinner-border spinner-border-sm"></span> Đang xử lý...');
        }
    });
});

// BMI Calculator Helper
function calculateBMI(height, weight) {
    const heightInMeters = height / 100;
    return (weight / (heightInMeters * heightInMeters)).toFixed(2);
}

// Format number with thousand separator
function formatNumber(num) {
    return num.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}
