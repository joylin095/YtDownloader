import sys
import yt_dlp
import os
import json

def download_file(youtube_url, output_dir):
    ydl_opts = {
        'format': 'bestaudio/best',
        'postprocessors': [{
            'key': 'FFmpegExtractAudio',
            'preferredcodec': 'mp3',
            'preferredquality': '192'
        }],
        'outtmpl': os.path.join(output_dir, '%(title)s.%(ext)s'),
        'quiet': True,
        'no_warnings': True
    }
    with yt_dlp.YoutubeDL(ydl_opts) as ydl:
        info_dict = ydl.extract_info(youtube_url, download=True)

        info = info_dict.get("entries",None)
        if info is None:
            result = {
                "Title": info_dict['title'],
                "Filepath": info_dict['requested_downloads'][0]['filepath']
            }
            return json.dumps(result,ensure_ascii = False)
        # for i in info:
        #     print(f"Downloaded: {i['title']}")
        #     print(f"Downloaded to: {i['requested_downloads'][0]['filepath']}")

if __name__ == "__main__":
    if len(sys.argv) < 3:
        print("Usage: download_list_ytdlp.py <url_list> <output_dir>")
        sys.exit(1)

    youtube_url = sys.argv[1]
    output_dir = sys.argv[2]

    if not os.path.exists(output_dir):
        os.makedirs(output_dir)

    try:
        download_file = download_file(youtube_url, output_dir)
        print(download_file)
    except Exception as e:
        print(f"Error: {e}")
        sys.exit(1)