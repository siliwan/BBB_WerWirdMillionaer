<template>
    <b-container class="home container">
        <b-row>
            <b-col>
                <p class="h1 text-center">Welcome to the "Wer Wird Millionär" Quiz</p>
            </b-col>
        </b-row>
        <b-row class="mb-1">
            <b-col>
                <b-img width="200%" height="200%" src="@/assets/images/WWM.png" fluid center rounded="circle" alt="Who wants to be a millionaire" />
            </b-col>
        </b-row>
        <b-row>
            <b-col class="text-center">
                <b-link class="btn btn-primary" to="/quiz/start" size="lg" block>Start Quiz</b-link>
            </b-col>
        </b-row>
    </b-container>
</template>

<script lang="ts"> 
    import { Category, HighScore, nameof, typedUnitialized } from '@/ResponseTypes';
    import { BModal, BNavForm, BvModal, BvModalEvent } from 'bootstrap-vue';
    import { Component, Prop, Vue } from 'vue-property-decorator';
    import CategoryCrud from "./CRUD/CategoryCrud"

    @Component({
        components: {
        }
    })
    export default class Home extends Vue {
        private categories: Category[] = [];
        private selected: Category = new Category();
        private fields = [
            { key: nameof<Category>("id"),   label: nameof<Category>("id").toUpperCase() },
            { key: nameof<Category>("name"), label: nameof<Category>("name").toUpperCase() },
        ]
        private isLoading: boolean = true;
        private state: boolean = true;

        mounted() {
            this.loadData();
        }


        show(item: any, index: any, button: any) {
            this.$root.$emit('bv::show::modal', this.selected.$id, button)
        }

        loadData() {
            this.isLoading = true;
            CategoryCrud.getAll()
                        .then(res => this.categories = res)
                        .catch(err => console.error(err))
                        .finally(() => this.isLoading = false)
        }

        onRowSelected(item: Category) {
            console.log(item)
            if(Array.isArray(item)) {
                item = item[0];
            }

            this.selected = item;
            this.modal.show()
        }

        async onSubmit(evt: BvModalEvent) {
            evt.preventDefault();

            await CategoryCrud.update(this.selected.id, this.selected.name);
            this.loadData();

            this.$nextTick(() => {
                this.$bvModal.hide('modal');
            })
        }

        async onDelete(evt: any) {
            await CategoryCrud.delete(this.selected.id);
            this.loadData();

            this.$nextTick(() => {
                this.$bvModal.hide('modal');
            })
        }

        get modal() {
            let m = this.$refs['modal'];

            return m as BModal;
        }
    }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>
