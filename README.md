# CASC  
> A compiler aim for Manderins progremmers  

CASC is a handwritten compiler which can compile English or Manderin or even mixed codes!  
Currently it's under developement by ChAoS_UnItY.  
  
This project is inspired by [Minsk](https://github.com/terrajobst/minsk).

## Example Code <br> 程式碼範例
------
```casc
> 1 + 9 - 7
3
> 一 加 九 減 七
3
> 1 + 九 減 7
3
> 一加二十一是二十二
True
```
## Preserved Word Conversion Table <br> 保留字對照表
------
### Operators 運算子
| Operator  | Traditional Chinese   | Simplified Chinese    | Note      |
|----------:|----------------------:|----------------------:|----------:|
| +         | 加 / 正               | TODO                  |
| -         | 減 / 負               | TODO                  |
| /         | 除                    | TODO                  |
| *         | 乘                    | TODO                  |
| .         | 點                    | TODO                  |
| (         | 開                    | TODO                  | TF        |
| )         | 閉                    | TODO                  | TF        |
| &&        | 且                    | TODO                  |
| \|\|      | 或                    | TODO                  |
| !         | 反                    | TODO                  |
| ==        | 是                    | TODO                  |
| !=        | 不是                  | TODO                  |
| ^2        | 平方                  | TODO                  | OUA / RF  |
| 2√        | 平方根                | TODO                  | OUA / RF  |
| ^^        | 次方                  | TODO                  | OUA / RF  |
| √         | 開方                  | TODO                  | OUA / RF  |

TF: Testing feature. 測試特性。  
OUA: Operator Unacceptable. 不接受純運算子，即不接受第一欄位。
RF: Removed feature. 移除特性。
