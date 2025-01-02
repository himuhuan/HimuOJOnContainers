import { MessageApiInjection } from "naive-ui";

declare global {
    interface Window {
        $message: MessageApiInjection;
        $mathjax: any;
    }
}