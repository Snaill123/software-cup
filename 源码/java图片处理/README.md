#TesseractOCR

## 系统环境搭建

1. 首先需要安装系统相关软件`Tesseract`, `JDK`或`JRE`
2. 软件安装完成后需要进行相应的配置
    2.1配置`Tesseract`环境
	在系统环境变量`Path`下添加`Tesseract`运行路径。如 "`[安装路径]\tesseract-ocr`";
	新建系统变量`TESSDATA_PREFIX`,对应的值为当前系统安装的`Tesseract`安装路径（同上）
	2.2配置Java运行环境
          在系统环境变量`Path`下添加`Java`运行路径
          新建系统变量`JAVA_HOME`和`JRE_HOME`,值分别为JDK安装目录和JRE安装


## 使用准备

1. 指定图片文件夹的位置
2. 确保`Tesseract`目录下有识别图片所需的语言包


## 编码规范

为提高团队协作开发效率，我们约定一些编码规范，希望各位认真阅读并遵守。

规范严谨的代码风格体现的是开发者的认真和专业，所以从这点出发，也需要养成一个好的代码风格习惯。

### Java

有待添加！

## 测试
有待添加！

## 开发工作流

1. 每次开发新任务时，切换到在`master`分支上，运行`git pull --rebase`或`git pull --rebase origin master`，从远程仓库拉取最新代码，来更新本地master；开发已进行的任务时不用执行此步骤
2. 拉取最新代码后不可以在`master`分支上进行开发，保持本地`master分支与远程仓库`master`一致，这样能避免不必要的麻烦
3. 新建分支`branchname-yourname`，如：`feature-zcq`。然后在新分支上开发自己的功能。分支命名通常用`-`来连接单词，建议以功能或者行为来命名。
4. 在自己的分支上可以多次`commit`，还可以push到远端（coding.net），以便下次可能在另一台设备开发以及防止本地文件丢失。
5. 当前分支功能开发完毕（包括业务逻辑开发和单元测试），测试通过后，运行`git fetch origin master`，`git rebase origin/master`，如果有冲突，谨慎解决。
6. 完成合并后，`git push`或`git push origin feature`到远端，在coding网页端申请合并到`master`分支（MR, Merge Request）。
7. 合并后远程会删掉这个`feature`分支，而你的工作也进入到下一阶段，`checkout`到`master`，重复第一步

注意：

有待添加！

## 部署

### 初始化

有待添加！

### 部署流程

有待添加！

