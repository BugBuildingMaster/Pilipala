
# Pilipala# Pilipala# Pilipala动漫网项目
## Pilipala Comic Web Project
- 噼里啪啦动漫网项目是一个工作室项目，由 **bugbuildingmaster**   **zkysnow** 以及  **无限权益合伙人 **编写，项目仅供参考学习，不得用于正规用途，任何其他用途需要至少得到三位作者中的任何一位同意
  联系方式：
  - **bugbuildingmaster**   ：332512139@qq.com
  - **zkysnow** ：1012469751@qq.com
  - **无限权益合伙人**：1097290837@qq.com
## 关于项目
- 使用ASP.NET开发框架，用于通过 HTML、CSS、JavaScript 以及服务器脚本来构建网页和网站
- 使用Vue.js技术搭建项目的后台管理系统，并通过IIS Express端口进行前后端数据交互
- 使用Python语言编写爬虫脚本，从其他网站获取初始数据
- 使用RSA非对称加密进行相关数据加密
## 环境要求
- 本项目使用**Microsoft Visual Studio 2019**编写，数据库引擎使用**Microsoft SqlServer 2019**

  ### 2021/5/11日志

  - 将前端含session内容修改，全局应用token

  ### 2021/5/12日志

  - 路径修改

  ### 2021/5/13日志

  - 修复注册的bug（没有调用相应的存储过程导致报错）
  - 登录注册页完成
  - 增加注册的反馈

  ### 2021/5/14日志

  - 修改注销后的alert
  - 登录注册添加了防抖
  - 增加了登录检查
  - 增加了悬浮提示

  ### 2021/5/15日志

  - 加入了redis管理令牌池，使未过期但被注销的token被识别并拦截
  - 修复了验证令牌的bug，5/15上午的版本错误较多，现基本修复
  
  ### 2021/5/16日志
  
  - 在后端加入了请求节流，单位时间同一ip同时访问次数过多会将超过的请求全部忽略
  - 当下认为redis令牌管理池应该使用令牌名为键名，以令牌对应私钥为键值
  
  ### 2021/5/18日志
  
  - 耗时两天，搭建了令牌池，实现了仅允许单机登录
  - 搭建了密钥对池，限制了密钥对生成的数量，达到上限时会使用已生成的密钥对
  
  ### 2021/5/19日志
  
  - 测试后修复了token验证的逻辑，实现了临期token更新
  - 增加了登录注册的加载页面
  - 增加系统健壮性，不让错误直接出现在页面上
  
  ### 2021/5/23日志
  
  - 将历史记录功能补充完整，逻辑重整，界面美化