function downloadMp3() {
    $('#downloadLink').hide();
    $('#downloadBtn').attr('disabled', true);
    const youtubeUrl = $('#youtubeUrl').val();

    if (!youtubeUrl) {
        alert('請輸入youtube影片網址');
        $('#downloadBtn').attr('disabled', false);
        return
    }

    $.ajax({
        url: "/api/Download",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify({ youtubeUrl: youtubeUrl }),
        success: function (response) {
            if (response.success) {
                $('#downloadLink').attr('href', response.downloadUrl).show();
            } else {
                alert('下載失敗');
            }
        },
        error: function (response) {
            alert('發生錯誤');
            console.log(response);
        },
        complete: function () {
            $('#downloadBtn').attr('disabled', false);
        }
    })
}