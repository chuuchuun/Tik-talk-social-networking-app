import { Message } from "./message.interface"
export interface Chat {
    id: number,
    userFirst: number,
    userSecond: number,
    lastMessage: string,
    messages : Message[]
}
