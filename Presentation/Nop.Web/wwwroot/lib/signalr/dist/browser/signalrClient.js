const signalR = require("@aspnet/signalr");
import $ from "jquery";
/**
 * Using Samples : 
 * 
 * var connection = new SignalR.create ('api/signalr/approval');
 * connection.start()
 * connection.stop()
 * connection.reconnect()
 * connection.send(msg)
 * connection.onClose( ()=>{ //Do something with onClose data}  )
 * connection.recieved( data => { //Do something with broadcasted data  } )
 * 
 */

/**
 * @class SignalR
 * @description SignalR allows bi-directional communication between server and client
 */
class SignalR {
    constructor(url) {
        this.connection = new signalR.HubConnectionBuilder().withUrl(url).build();
        /**
         * Connection States
         */
        this.state = {
            connecting: 0,
            connected: 1,
            reconnecting: 2,
            disconnected: 4
        }

        this.hasRecived = false;
    }

    /**
     * Get current connection state
     */
    getState() {
        return this.state.connection.connection.connectionState;
    }

    /**
     * Reconnect to signarR server
     * 
     * @description Trying reconnect to signalR server every 2 seconds
     */
    reconnect() {
        var self = this;
        var reconnectTimeout = setInterval(() => {
            if (self.getState(self.start()) == self.state.connected) {
                console.log('سیستم موفق به اتصال مجدد شد')
                clearInterval(reconnectTimeout);
            } else {
                console.log('در حال تلاش بoرای اتصال مجدد')
            }
        }, 2000)

    }
    /**
     * OnClose work when signalR disconnected
     * 
     * @param {Function} cb is callback function
     */
    onClose(cb) {
        this.connection.onclose(data => cb.call(data.message));
        return this;
    }

    /**
     * Recieve data from signalR
     * 
     * @param {Function} cb is client callback when receive data
     */

    recieved(name, cb, multipleListener) {
        //Prevent multitple listener
        if (this.hasRecived == true && multipleListener == false ){
            return
        }

        this.hasRecived = true;
        this.connection.on(name, msg => {
            var msg = JSON.parse(msg);
            cb.call(this, msg)
        });
        return this;
    }

    /**
     * Get current connection state
     */
    getState() {
        return this.connection.connection.connectionState;
    }

    /**
     * Check & notify state changes
     */
    checkState() {
        if (this.getState() == this.state.connecting) {
            console.warn('connecting');
        } else if (this.getState() == this.state.connected) {
            console.warn('connected');
        } else if (this.getState() == this.state.reconnecting) {
            console.warn('reconnecting');
        } else if (this.getState() == this.state.disconnected) {
            console.warn('disconnected');
        }
    }

    /**
     * Start signalr keep alive connection
     * 
     * @param {Function} cb is callback
     */
    start(cb) {
        if (cb) cb.call(this);

        this.connection.start().catch(err => console.error(err.toString()));
        return this;
    }

    /**
     * Stop signalr keep alive connection
     * 
     * @param {Function} cb is callback
     */
    stop(cb) {
        if (cb) cb.call(this)

        this.connection.stop().catch(err => console.error(err.toString()));
        return this;
    }

    /**
     * Send serialized json message to server 
     * 
     * @param {String} msg is serialized json data
     */
    send(name, msg) {
        if (this.connection.connection.connectionState != this.state.connected)
            throw new Error("SignalR: Connection hasn't been started yet");

        if(typeof(meg) == "Object"){
            msg = JSON.stringify(msg);
        }
        this.connection.invoke(name, msg).catch(err => console.error(err.toString()));
        return this;
    }
}

export default {
    create: function (config) {
        return new SignalR(config)
    }
}