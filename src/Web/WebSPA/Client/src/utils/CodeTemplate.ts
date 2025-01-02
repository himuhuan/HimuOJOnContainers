export class CodeTemplate {
    private _defaultCodeTemplateMap: Map<string, string>;

    public constructor() {
        this._defaultCodeTemplateMap = new Map([
            ["cpp", "#include <iostream>\nusing namespace std;\n\nint main() {\n\t// your code here\n\treturn 0;\n}"],
            ["c", "#include <stdio.h>\n\nint main() {\n\t// your code here\n\treturn 0;\n}"],
            ["java", "public class Main {\n\tpublic static void main(String[] args) {\n\t\t// your code here\n\t}\n}"],
            ["python", "# your code here\n"],
            ["_new_problem", "请不要在问题描述内重复您的标题，在此处使用 Markdown 语法编写您的问题。"
                             + "`HimuOJ` 支持绝大多数Markdown 功能。一般而言推荐的模板如下: \n"
                             + "## 问题描述\n\n## 输入格式\n\n## 输出格式\n\n## 样例\n\n## 说明/提示\n\n"],
        ]);
    }

    public getTemplate(language: string): string {
        if (this._defaultCodeTemplateMap.has(language)) {
            return this._defaultCodeTemplateMap.get(language) as string;
        } else
            throw new Error("Language not supported");
    }

    public setTemplate(language: string, template: string) {
        this._defaultCodeTemplateMap.set(language, template);
    }
}

export const CodeTemplateInstance = new CodeTemplate();