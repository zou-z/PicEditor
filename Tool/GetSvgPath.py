# 提取SVG的path
import os
import re
import requests

def GetSvgPath(svg):
    return re.findall('<path.*?d="(.*?)"', svg, re.S)

def HttpGet(url):
    session = requests.session()
    headers = {
        "User-Agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/102.0.5005.124 Safari/537.36 Edg/102.0.1245.44"
    }
    response = session.get(url, headers = headers)
    return response.text


if __name__ == "__main__":
    print("[提取SVG的path]")
    while True:
        try:
            print("1.SVG代码")
            print("2.SVG文件路径")
            print("3.SVG文件链接")
            print("4.退出")
            mode = input("请输入打开方式：")
            svg = ""
            if mode == "1":
                svg = input("请输入SVG代码：")
            elif mode == "2":
                svg = input("请输入SVG文件路径：")
                if os.path.exists(svg) and os.path.isfile(svg):
                    with open(svg, "r") as f:
                        svg = f.read()
            elif mode == "3":
                svg = input("请输入SVG文件链接：")
                svg = HttpGet(svg)
            elif mode == "4":
                break
            else:
                print("输入错误\n")
                continue
            pathList = GetSvgPath(svg)
            print("共有{0}个path".format(len(pathList)))
            for path in pathList:
                print("--------------------")
                print(path)
            print()
        except Exception as e:
            print(e)
