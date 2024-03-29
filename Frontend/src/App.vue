<template>
    <div id="app">
        <b-navbar toggleable="lg" variant="dark" type="dark">
            <b-navbar-brand>
                Wer wird Millionär
            </b-navbar-brand>
            <b-navbar-toggle target="nav-collapse"></b-navbar-toggle>
            <b-collapse id="nav-collapse" is-nav>
                <b-navbar-nav>
                    <b-nav-item to="/">Home</b-nav-item>
                    <b-nav-item to="/highscores">Highscores</b-nav-item>
                    <b-nav-item v-if="isAuthenticated" to="/questions">Questions</b-nav-item>
                    <b-nav-item v-if="isAuthenticated" to="/categories">Categories</b-nav-item>
                </b-navbar-nav>
                <b-navbar-nav class="ml-auto">
                    <b-button variant="danger" v-if="showClearStorage" @click="onClear">Clear storage</b-button>
                    <b-nav-item v-if="!isAuthenticated" to="/profile/login">Login</b-nav-item>
                    <b-nav-item-dropdown v-if="isAuthenticated" :text="user.username != undefined ? user.username : '{{ user }}'" right>
                        <b-dropdown-item to="/profile">Profile</b-dropdown-item>
                        <b-dropdown-item @click="onLogout">Logout</b-dropdown-item>
                    </b-nav-item-dropdown>
                </b-navbar-nav>
            </b-collapse>
        </b-navbar>
        <b-breadcrumb :items="breadcrumb"></b-breadcrumb>
        <router-view />
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Watch } from 'vue-property-decorator';
    import Home from './components/Home.vue';
    import { nameof, User } from './ResponseTypes';
    import AuthApi from './services/AuthApi';

    @Component({
        components: {
            Home
        }
    })
    export default class App extends Vue {
        private user: User = new User();
        private transitionName: string = 'slide-right'; 

        onClear(evt: PointerEvent) {
            localStorage.removeItem('x-quiz-session-id');
            try {
                this.$router.push('/')
            } catch (error) {
                
            }
        }

        async mounted() {
            this.user = await AuthApi.LoginInformation();
        }

        async onLogout() {
            try {
                if(await AuthApi.IsLoggedIn()) {
                    await AuthApi.Logout();
                    this.user = new User();
                } 
            } finally {
                window.location.reload();
            }
        }

        get breadcrumb() {
            console.log(this.$route.fullPath)
            return this.$route.fullPath.split('/').map((fragment, index, arr) => {
                return {
                    text: index == 0 ? 'Home' : fragment,
                    href: index == 0 ? '/' : arr.slice(0, index + 1).join('/'),
                    active: arr.length -1 == index
                }
            });
        }

        get isAuthenticated() {
            return this.user.id !== undefined;
        }

        get showClearStorage() {
            return localStorage.getItem('debug')?.trim() ?? 'false' !== 'false';
        }

        @Watch(nameof<App>('$route'))
        async onRouteChanged(to: any, from: any) {
            console.log("Route changed to " + to.path)
            if(to.path === '/profile' && await AuthApi.IsLoggedIn()) {
                this.user = await AuthApi.LoginInformation()
            }

            const toDepth = to.path.split('/').length
            const fromDepth = from.path.split('/').length
            this.transitionName = toDepth < fromDepth ? 'slide-right' : 'slide-left'
        }
    }
</script>

<style>
    .slide-right-enter-active,
    .slide-left-leave-active {
    transition: all 0.05s ease-out;
    }


    .slide-left-enter-to {
    position: absolute;
    right: 0;
    }

    .slide-right-enter-to {
    position: absolute;
    left: 0;
    }


    .slide-left-enter-from {
    position: absolute;
    right: -100%;
    }

    .slide-right-enter-from {
    position: absolute;
    left: -100%;
    }


    .slide-left-leave-to {
    position: absolute;
    left: -100%;
    }

    .slide-right-leave-to {
    position: absolute;
    right: -100%;
    }


    .slide-left-leave-from {
    position: absolute;
    left: 0;
    }

    .slide-right-leave-from {
    position: absolute;
    left: 0;
    }
</style>
