# 简介
扩展反射出的元数据,能够获取相关的注释

### 支持的类
- Type
- MethodInfo
- ParameterInfo
- PropertyInfo

### 用法
```csharp
/// <summary>
/// 测试类
/// </summary>
public class TestType
{
}

MetadataXmlCommentExtension.InculudeXml(GetXmlPath());
var type = typeof(TestType);
var summary = type.GetComments() //summary=测试类
```
