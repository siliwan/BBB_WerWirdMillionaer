<template>
    <b-container>
        <b-row>
            <b-col>
                <h1>Login</h1>
            </b-col>
        </b-row>
        <b-form @submit="onSubmit">
            <b-form-row>
                <b-col>
                    <b-form-group label="Username" label-for="model_username">
                        <b-form-input id="model_username" ref="model_username" v-model="model.username" required autocomplete="username"/>
                    </b-form-group>
                </b-col>
            </b-form-row>
            <b-form-row>
                <b-col>
                    <b-form-group label="Password" label-for="model_password">
                        <b-form-input id="model_password" ref="model_password" v-model="model.password" required type="password" autocomplete="current-password"/>
                    </b-form-group>
                </b-col>
            </b-form-row>
            <b-form-row>
                <b-col>
                    <b-button type="submit" variant="primary">
                        Login
                    </b-button>
                </b-col>
            </b-form-row>
        </b-form>
    </b-container>
</template>
<script lang="ts">
    import { UserPassword } from '@/ResponseTypes';
    import AuthApi from '@/services/AuthApi';
    import { Component, Vue } from 'vue-property-decorator';

    type LoginActionMethod = 'login' | 'logout' | 'invalid';

    @Component({
        components: {
            
        }
    })
    export default class Login extends Vue {

        private model: UserPassword = new UserPassword();

        async mounted() {
            if(this.method == 'invalid') {
                this.$router.go(-1);
            }

            if(this.method == 'logout') {
                
                try {
                    if(await AuthApi.IsLoggedIn()) {
                        await AuthApi.Logout();
                    }
                } finally {
                    this.$router.push('/');
                }
                
            }

            if(this.method == 'login') {
                if(await AuthApi.IsLoggedIn()) {
                    this.$router.push('/profile');
                }
            }
        }

        async onSubmit(evt: Event) {
            evt.preventDefault();

            if(!this.isValid()) {
                return;
            }

            let result = await AuthApi.Login(this.model);
            if(!result.success) {
                this.$bvToast.toast(result.error, {
                    title: "Login failed",
                    variant: 'danger'
                });
            } else {
                this.$router.push('/profile');
            }
  
        }

        isValid() {
            let isValid = true;

            if(this.model.username === undefined) isValid = false
            if(this.model.username.trim() === '') isValid = false
            if(this.model.password === undefined) isValid = false
            if(this.model.password.trim() === '') isValid = false

            return isValid;
        }

        get method(): LoginActionMethod {
            
            switch (this.$route.params['method']) {
                case 'login':
                    return 'login';

                case 'logout':
                    return 'logout';

                default:
                    return 'invalid';
            }
        }

    }

</script>
<style scoped>

</style>