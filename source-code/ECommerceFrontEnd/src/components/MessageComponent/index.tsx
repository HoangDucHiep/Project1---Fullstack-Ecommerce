export const MessageComponent = {
    success: (content: string) => {
        (window as any).__messageApi?.open({
            type: "success",
            content,
            duration: 4,
        });
    },
    error: (content: string) => {
        (window as any).__messageApi?.open({
            type: "error",
            content,
            duration: 4,
        });
    },
    info: (content: string) => {
        (window as any).__messageApi?.open({
            type: "info",
            content,
            duration: 4,
        });
    },
};
