# 提取SVG的path
import os
import re

def GetSvgPath(svg):
    return re.findall('<path.*?d="(.*?)"', svg, re.S)

if __name__ == "__main__":
    print("[提取SVG的path]")
    while True:
        try:
            svg = input("请输入Svg文件路径或Svg代码(exit退出)：")
            if svg == "exit":
                break
            if os.path.exists(svg) and os.path.isfile(svg):
                with open(svg, "r") as f:
                    svg = f.read()
            pathList = GetSvgPath(svg)
            print("共有{0}个path".format(len(pathList)))
            for path in pathList:
                print("--------------------")
                print(path)
            print()
        except Exception as e:
            print(e)
