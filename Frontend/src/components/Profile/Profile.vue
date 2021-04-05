<template>
    <b-container>
        <b-row>
            <b-col>
                <h1>Hello, {{ user.username }}</h1>
            </b-col>
        </b-row>
        <b-row>
            <b-col>
                <b-button variant="primary" to="/questions">Go to questions</b-button>
            </b-col>
            <b-col>
                <b-button variant="primary" to="/categories">Go to categories</b-button>
            </b-col>
            <b-col>
                <b-button variant="primary" @click="onLogout">Logout</b-button>
            </b-col>
        </b-row>
    </b-container>
</template>
<script lang="ts">
    import { User } from '@/ResponseTypes';
    import AuthApi from '@/services/AuthApi';
    import { Component, Vue } from 'vue-property-decorator';

    @Component({
        components: {
            
        }
    })
    export default class Profile extends Vue {

        private user: User = new User()

        async mounted() {
            let isAuthenticated = await AuthApi.IsLoggedIn();

            if(!isAuthenticated) {
                this.$router.push('/profile/login');
            }

            this.user = await AuthApi.LoginInformation();
        }

        async onLogout() {
            try {
                await AuthApi.Logout();
            } finally {
                this.$router.push("/");
            }
        }
    }
</script>

<style scoped>

</style>