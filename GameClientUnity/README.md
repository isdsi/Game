asset diff가 체크되지 않을 경우 연결된 git의 다음 설정을 변경할 것
git config --global merge.unityyamlmerge.name "Unity SmartMerge"
git config --global merge.unityyamlmerge.driver "\"C:/Program Files/Unity/Hub/Editor/2022.3.62f3/Editor/Data/Tools/UnityYAMLMerge.exe\" merge -p --\"%O\" --\"%B\" --\"%A\""

