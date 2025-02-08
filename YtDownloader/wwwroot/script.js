function downloadMp3() {
    const youtubeUrl = $('#youtubeUrl').val();

    if (!youtubeUrl) {
        alert('請輸入youtube影片網址');
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
            console.log(response);
        }
    })
}