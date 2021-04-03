<template>
    <div class="timer">
      <p>{{ minutes }}:{{ seconds }}</p>
    </div>
</template>
<script lang="ts">

    import { Component, Vue, Prop, Emit } from 'vue-property-decorator';

    @Component({
        components: {
            
        }
    })
    export default class Countdown extends Vue {
      private handle!: number; 

      @Prop(Date) CountdownUntil!: Date;

      private now: Date = new Date();

      mounted() {
        this.handle = setInterval(this.onInterval, 1000)
      }

      onInterval() {
        this.now = new Date();
        if(this.CountdownUntil < this.now) {
          this.onCompleted();
          clearInterval(this.handle);
          this.handle = -1;
        }
      }

      public get minutes() {
        let min = new Date(this.CountdownUntil.getTime() - this.now.getTime()).getMinutes();

        if(min < 0) {
          min = 0;
        }

        return min.toString().padStart(2, "0");
      }

      public get seconds() {
        let sec = new Date(this.CountdownUntil.getTime() - this.now.getTime()).getSeconds();
        
        if(sec < 0) {
          sec = 0;
        }

        return sec.toString().padStart(2, "0");
      }

      @Emit('onCompleted')
      onCompleted() {

      }

    }

</script>

<style>
</style>
