import Vue from 'vue';
import App from './App.vue';
import { BootstrapVue, IconsPlugin } from 'bootstrap-vue'
import 'bootstrap/dist/css/bootstrap.css'
import 'bootstrap-vue/dist/bootstrap-vue.css'
import VueRouter from 'vue-router';
import router from '@/router'
import AuthPlugin from '@/services/AuthPlugin'

Vue.use(BootstrapVue)
Vue.use(IconsPlugin)
Vue.use(VueRouter)
Vue.use(AuthPlugin);
Vue.config.productionTip = true;

new Vue({
    render: h => h(App),
    router
}).$mount('#app');