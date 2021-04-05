<template>
    <div class="container border p-4">
        <b-alert :show="hasWarning" variant="danger">{{warning}}</b-alert>
        <div class="container-fluid" v-if="!hasWarning && !isLoading">
            <div class="row">
                <p class="h2"><router-link to="/highscores"><b-icon icon="arrow-left"></b-icon></router-link></p>
            </div>
            <div class="row">
                <div class="col">
                    <h2>Rank #{{ rankFormatted }}</h2>
                </div>
            </div>
            <div class="row">
                <div class="col">
                    <h2>by {{ highscore.name }}</h2>
                </div>
            </div>
            <div class="row">
                <div class="col">
                    Points: {{ highscore.pointsWeighted }}
                </div>
                <div class="col">
                    Duration: {{ durationFormatted }}
                </div>
            </div>
            <div class="row" v-if="isAuthenticated">
                <div class="col">
                    <b-button variant="danger" @click="onDelete">Delete</b-button>
                </div>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
    import HighscoreCrud from '@/components/CRUD/HighscoreCrud';
    import AuthApi from '@/services/AuthApi';
    import { HighScore } from '@/ResponseTypes';
    import { Component, Vue } from 'vue-property-decorator';

    @Component({
        components: {
            
        }
    })
    export default class HighscoreDetail extends Vue {
        
        private isAuthenticated: boolean = false;

        private isLoading: boolean = true;

        private warning = '';
        private highscore: HighScore = new HighScore();

        constructor() {
            super();
        }

        async mounted() {
            try {
                let id = Number(this.$route.params.id);
                this.highscore = await HighscoreCrud.get(id);

                if(this.highscore === undefined || this.highscore === null) {
                    this.warning = 'Invalid highscore';
                }

                this.isLoading = false;

                this.isAuthenticated = await AuthApi.IsLoggedIn();

            } catch (error) {
                this.warning = 'Invalid highscore';
            }
        }

        async onDelete() {
            if(!this.isAuthenticated) {
                return;
            }

            if(confirm('Do you really want to delete this highscore?')) {
                try {
                    await HighscoreCrud.delete(this.highscore.id);
                    this.$router.push('/')
                } catch (error) {
                    this.warning = "Could not delete the entry.";
                }
            }
        }

        get rankFormatted() {
            if(this.highscore.id !== undefined) {
                switch (this.highscore.rank) {
                    case 1:
                        return "1 🥇";

                    case 2:
                        return "2 🥈";

                    case 3:
                        return "3 🥉";
                
                    default:
                        return this.highscore.rank;
                }
            }

            return "-1";
        }

        get durationFormatted() {
            if(this.highscore.id !== undefined) {
                let time = new Date(0);
                time.setSeconds(this.highscore.duration);
                console.log(time)
                return `${time.getMinutes().toString()}m ${time.getSeconds().toString()}s`;
            }

            return "0m 0s";
        }

        get hasWarning() {
            return this.warning !== undefined && this.warning.trim() !== '';
        }
    }
</script>

<style scoped>

</style>